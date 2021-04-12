export default {
	home: {
		"label": "HomePage",
		"name": "Home",
		"to": "",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		menus: [
			{
				"label": "Panel",
				"to": "/panel",
				icon: "dashboard",
				"isHeaderVisible": true,
				//roles: ["Reader", "Writer", "Admin"],
				"menus": []
			},
			// {
			// 	"label": "Management",
			// 	"to": "/admin",
			// 	icon: "work",
			// 	showOnAuth: true,
			// 	"isHeaderVisible": true,
			// 	roles: ["Admin"],
			// 	"menus": []
			// },
			{
				isDivider: true
			},
			{
				"label": "Swagger",
				"to": "/swagger",
				icon: "code",
				"isHeaderVisible": true,
				"target": "_blank",
				//roles: ["Admin", "Writer"],
				"menus": []
			},
			// {
			// 	"label": "OAS3",
			// 	"to": "/swagger/v1/swagger.json",
			// 	icon: "code",
			// 	"isHeaderVisible": true,
			// 	"target": "_blank",
			// 	"menus": []
			// },
			{
				"label": "Files",
				name: "Files",
				"to": "/files",
				icon: "folder_open",
				"isHeaderVisible": true,
				"target": "_blank",
				"menus": []
			},
			{
				"label": "Jobs",
				name: "Jobs",
				"to": "/hangfire",
				"icon": "engineering",
				"isHeaderVisible": true,
				"target": "_blank",
				//roles: ["Admin", "Writer"],
				"menus": []
			},
			{
				"label": "Help",
				name: "Help",
				"to": "/help",
				icon: "help_outline",
				"isHeaderVisible": true,
				"menus": []
			}
		]
	},
	panel: {
		label: "Panel",
		name: "panel",
		to: "/panel",
		areMenusVisible: true,
		isHeaderVisible: false,
		menus: [
			{ label: "Definitions", to: "", name: "Definitions", areMenusVisible: true, isHeaderVisible: true, dividerBefore: true, menus: [] },
			{ label: "Settings", to: "", name: "Settings", areMenusVisible: true, isHeaderVisible: true, dividerBefore: true, roles: ["Admin"], menus: [] },
		]
	},
	user: {
		"label": "Account",
		"name": "user",
		"to": "",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		"menus": [
			{
				"label": "Login",
				"to": "/auth/login",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				showOnAuth: false,
				"menus": []
			},
			// {
			//   "label": "Kayıt",
			//   "to": "/auth/register",
			//   "areMenusVisible": true,
			//   "isHeaderVisible": true,
			//   showOnAuth: false,
			//   "menus": []
			// },
			{
				"label": "Account Settings",
				"to": "/auth/account",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				showOnAuth: true,
				"menus": []
			},
			{
				"label": "Logout",
				"to": "/auth/logout",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				showOnAuth: true,
				"menus": []
			}
		]
	},
	contact: {
		"label": "Contact",
		"name": "Contact",
		"to": "/contact",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		"menus": [
			{
				"label": "Links",
				"name": "Links",
				"to": "/links",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": [
					{
						"label": "Services",
						"to": "/services",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					},
					{
						"label": "FAQ",
						"to": "/faq",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					},
					{
						isDivider: true,
					},
					{
						"label": "Privacy Policy",
						"to": "/privacy-policy",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					},
					{
						"label": "Terms of Use and Service",
						"to": "/Terms-of-Use-and-Service",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					},
					{
						"label": "KVKK",
						"to": "/kvkk-clarification-text",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					},
					{
						"label": "About",
						"to": "/about",
						"areMenusVisible": true,
						"isHeaderVisible": true,
						"menus": []
					}
				]
			}
		],
		"additionalInfo": {
			"name": "CompanyInfo",
			"companyName": "DNA Proje ve Yazılım",
			"companyWebsite": "https://dna.com.tr",
			"companyWebsiteUrl": "https://dna.com.tr",
			"companyLogo": "",
			"address": [
				"",
				"",
				"Ataşehir/İstanbul"
			]
		}
	},
	main: {
		"label": "Main Menu",
		"name": "Main",
		"to": "/main",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		"menus": []
	},
	social: {
		"label": "Social",
		"name": "Social",
		"to": "/social",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		"menus": [
			{
				"label": "Twitter",
				"to": "https://twitter.com/DNAProje",
				"icon": "Twitter",
				"color": "#55acee",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "YouTube",
				"to": "https://www.youtube.com/channel/UCg5Ibu_Gkk_67D0GAVADiZQ",
				"icon": "YouTube",
				"color": "#cd201f",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "Facebook",
				"to": "https://facebook.com/DNAProje",
				"icon": "Facebook",
				"color": "#3b5999",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "Instagram",
				"to": "https://www.instagram.com/dnaproje/",
				"icon": "Instagram",
				"color": "#e4405f",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "LinkedIn",
				"to": "https://www.linkedin.com/company/10004141",
				"icon": "LinkedIn",
				"color": "#0077B5",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			// {
			//   "label": "WhatsApp",
			//   "to": "tel:090...",
			//   "icon": "WhatsApp",
			//   "color": "#25D366",
			//   "areMenusVisible": true,
			//   "isHeaderVisible": true,
			//   "menus": []
			// },
			{
				"label": "Email",
				"to": "mailto:bilgi@dna.com.tr",
				"icon": "Mail",
				"color": "#dd4b39",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			}
		]
	},
	links: {
		"label": "Links",
		"name": "Links",
		"to": "/links",
		"areMenusVisible": true,
		"isHeaderVisible": true,
		"menus": [
			{
				"label": "Services",
				"to": "/services",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "FAQ",
				"to": "/faq",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				isDivider: true,
			},
			{
				"label": "Privacy Policy",
				"to": "/privacy-policy",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "Terms of Use and Service",
				"to": "/Terms-of-Use-and-Service",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "KVKK",
				"to": "/kvkk-clarification-text",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			},
			{
				"label": "About",
				"to": "/about",
				"areMenusVisible": true,
				"isHeaderVisible": true,
				"menus": []
			}
		]
	},
	admin: {
		label: "Management Panel",
		name: "admin",
		to: "/admin",
		areMenusVisible: true,
		isHeaderVisible: true,
		showOnAuth: true,
		roles: ["Admin"],
		menus: [
			{
				label: "Management",
				name: "management",
				to: "/management",
				menus: []
			},
			{
				label: "Araçlar",
				name: "Tools",
				to: "/tools",
				areMenusVisible: true,
				isHeaderVisible: true,
				menus: [
					{
						label: "Json to C#",
						name: "JsonToCSharp",
						to: "/json-to-c-sharp",
						icon: "code",
						menus: []
					}
				]
			}
		]
	},
	help: {
		label: "Help",
		name: "help",
		to: "/help",
		areMenusVisible: true,
		isHeaderVisible: true,
		menus: [
			{
				label: "AlertCodes",
				name: "alertCodes",
				to: "/alert-codes",
				icon: "warning",
				areMenusVisible: true,
				isHeaderVisible: true,
				description: "Alert Codes",
				menus: []
			}
		]
	}
}
