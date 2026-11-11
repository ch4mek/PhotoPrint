import { MainFormField } from '../constants';

import { Currency } from '@/constants/currency';

export const NOT_UNIQUE_SERIAL_NUMBER_ERROR = 'Serial number is already used';
export const NOT_UNIQUE_INVENTORY_NUMBER_ERROR =
	'Inventory number is already used';
export const TEXT_AREA_MIN_ROWS = 5;

export const MAIN_FORM_FIELDS = {
	equipmentName: {
		label: 'Equipment name',
		name: MainFormField.NAME,
	},
	equipmentType: {
		label: 'Type',
		name: MainFormField.TYPE,
		valueField: 'fullName',
	},
	status: {
		label: 'Status',
		name: MainFormField.STATUS,
	},
	invoiceNumber: {
		label: 'Invoice number',
		name: MainFormField.INVOICE_NUMBER,
	},
	serialNumber: {
		label: 'Serial number',
		name: MainFormField.SERIAL_NUMBER,
	},
	inventoryNumber: {
		label: 'Inventory number',
		name: MainFormField.INVENTORY_NUMBER,
	},
	location: {
		label: 'Location',
		name: MainFormField.LOCATION,
		valueField: 'description',
	},
	purchaseDate: {
		label: 'Purchase date',
		name: MainFormField.PURCHASE_DATE,
	},
	guaranteeDate: {
		label: 'Guarantee date',
		name: MainFormField.GUARANTEE_DATE,
	},
	approver: {
		label: 'Approver',
		name: MainFormField.APPROVER,
		valueField: 'fullName',
	},
	storage: {
		label: 'Storage',
		name: MainFormField.STORAGE,
	},
	purchasePrice: {
		label: 'Purch. price',
		name: MainFormField.PURCHASE_PRICE,
		currency: 'purchaseCurrency',
	},
	purchasePriceUsd: {
		label: 'Price, $',
		name: MainFormField.PRICE_USD,
		currency: 'purchaseCurrencyUsd',
		options: [Currency.USD],
	},
	purchasePlace: {
		label: 'Purchase place',
		name: MainFormField.PURCHASE_PLACE,
	},
	repairPlace: {
		label: 'Repair place',
		name: MainFormField.REPAIR_PLACE,
	},
	characteristics: {
		label: 'Equipment characteristics',
		name: MainFormField.CHARACTERISTICS,
	},
	jiraTask: {
		label: 'Jira task',
		name: MainFormField.JIRA_TASK,
	},
	comment: {
		label: 'Notes',
		name: MainFormField.COMMENT,
	},
};

export const REPAIR_PLACE_REQUIRED_STATUSES = ['Renovation'];
export const FORM_NAME = 'Equipment main form';
