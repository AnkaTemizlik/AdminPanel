
import CustomStore from "devextreme/data/custom_store";
import DataSource from 'devextreme/data/data_source'
import ArrayStore from 'devextreme/data/array_store';
import { toQueryString, isNotEmpty, addQueryFilter } from "../../../store/utils";

const createCustomStore = (options, key, defaultFilter, lists, onError) => {

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
							onError && onError(status)
							throw status.Message
						}
					})
					.catch((e) => {
						console.error("ERROR from catch", e)
						onError && onError(e)
						throw e
					});
			},
			insert: async (values) => {
				var status = await options.insert(values)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable insert ERROR", status)
					onError && onError(status)
					throw new Error(status.Message)
				}
			},
			update: async (key, values) => {
				var status = await options.update(key, values)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable update ERROR", status)
					onError && onError(status)
					throw new Error(status.Message)
				}
			},
			remove: async (key) => {
				var status = await options.delete(key)
				if (status.Success) {
					return true
				} else {
					console.error("DataTable remove ERROR", status)
					onError && onError(status)
					throw new Error(status.Message)
				}
			},
			byKey: (x) => { }
		})
}

export default createCustomStore