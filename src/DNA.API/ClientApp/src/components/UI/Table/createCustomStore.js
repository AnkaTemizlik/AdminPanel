
import CustomStore from "devextreme/data/custom_store";
import DataSource from 'devextreme/data/data_source'
import ArrayStore from 'devextreme/data/array_store';
import { toQueryString, isNotEmpty, addQueryFilter } from "../../../store/utils";

const createCustomStore = (options, key, defaultFilter, lists) => {

	return options.type == "simpleArray" // test amaçlı yaptım, henüz örneği yok. (editing enabled olmayanlarda kullanılabilir.)
		? new DataSource({
			store: new ArrayStore({
				key: options.key || "Id",
				data: lists[options.name]
			})
		})
		: new CustomStore({
			key: key || "Id",
			load: (loadOptions) => {

				let params = options.params ? { ...options.params } : {};
				[
					'skip',
					'take',
					'requireTotalCount',
					//'requireGroupCount',
					'sort',
					'filter'
					// 'totalSummary',
					// 'group',
					// 'groupSummary'
				].forEach(i => {
					if (i in loadOptions && isNotEmpty(loadOptions[i])) {
						params[i] = loadOptions[i]
					}
				});

				params["page"] = params.skip > 0 ? params.skip / params.take : 0

				if (defaultFilter) {
					params.filter = addQueryFilter(params.filter, defaultFilter)
				}

				console.warning("loadOptions", params, options, loadOptions)
				console.info("loadOptions", params)
				return options.load(toQueryString(params))
					.then((status) => {
						if (status.Success) {
							console.success("SUCCESS load ", status)
							return {
								data: status.Resource.Items,
								totalCount: status.Resource.TotalItems,
								// summary: null,
								// groupCount: null
							}
						} else {
							console.error("ERROR from status", status)
							options.onError && options.onError(status)
							throw status.Message
						}
					})
					.catch((e) => {
						console.error("ERROR from catch", e)
						options.onError && options.onError(e)
						throw e
					});
			},
			insert: async (values) => {
				var status = await options.insert(values)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable insert ERROR", status)
					options.onError && options.onError(status)
					throw status.Message
				}
			},
			update: async (key, values) => {
				var status = await options.update(key, values)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable update ERROR", status)
					options.onError && options.onError(status)
					throw status.Message
				}
			},
			remove: async (key) => {
				var status = await options.delete(key)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable remove ERROR", status)
					options.onError && options.onError(status)
					throw status.Message
				}
			}
		})
}

export default createCustomStore