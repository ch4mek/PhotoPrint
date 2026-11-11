import { LocationTypeId } from '@/types/configuration';

export enum EquipmentTabs {
	EQUIPMENT,
	USER,
}

export enum HistoryTabs {
	HISTORY,
	DELETED,
}

export const HistoryTabsData = [
	{
		id: HistoryTabs.HISTORY,
		text: 'History',
	},
	{
		id: HistoryTabs.DELETED,
		text: 'Deleted history',
	},
];

export const COLUMN_GAP = 8;
const COLUMNS_AMOUNT = 8;

export enum StatusEquipment {
	AVAILABLE = 'Available',
	BROKEN = 'Broken',
	RENOVATION = 'Renovation',
	WRITTEN_OFF = 'Written off',
	ISSUED = 'Issued',
	NONE = 'None',
}

export const ASSIGNED_EQUIPMENT_STATUS_OPTIONS = [
	StatusEquipment.ISSUED,
	StatusEquipment.BROKEN,
	StatusEquipment.RENOVATION,
] as string[];

export const STORAGE_OPTIONS = [
	LocationTypeId.Minsk,
	LocationTypeId.Vitebsk,
	LocationTypeId.Warsaw,
];

const MAIN_FORM_GRID_INPUT_WIDTH = '350px';
const MAIN_FORM_GRID_SMALL_INPUT_WIDTH_LEFT = '166px';
const MAIN_FORM_GRID_SMALL_INPUT_WIDTH_MIDDLE = '166px';
const MAIN_FORM_GRID_SMALL_INPUT_WIDTH_INVENTORY = '204px';
const MAIN_FORM_GRID_SECOND_ROW_EXTRA = '194px';
const MAIN_FORM_GRID_SMALL_TABLET_INPUT_WIDTH = '170px';

export enum MainFormField {
	NAME = 'name',
	TYPE = 'equipmentType',
	STATUS = 'status',
	INVOICE_NUMBER = 'invoiceNumber',
	SERIAL_NUMBER = 'serialNumber',
	INVENTORY_NUMBER = 'inventoryNumber',
	LOCATION = 'location',
	THIRD_ROW = 'thirdRow',
	FOURTH_ROW = 'fourthRow',
	CHARACTERISTICS = 'characteristics',
	COMMENT = 'comment',
	PURCHASE_DATE = 'purchaseDate',
	GUARANTEE_DATE = 'guaranteeDate',
	APPROVER = 'approver',
	STORAGE = 'storage',
	PURCHASE_PRICE = 'purchasePrice',
	PRICE_USD = 'purchasePriceUsd',
	PURCHASE_PLACE = 'purchasePlace',
	REPAIR_PLACE = 'repairPlace',
	JIRA_TASK = 'jiraTask',
}

export const NULLABLE_FIELDS = [
	MainFormField.PURCHASE_DATE,
	MainFormField.GUARANTEE_DATE,
	MainFormField.TYPE,
	MainFormField.LOCATION,
	MainFormField.STATUS,
	MainFormField.APPROVER,
	MainFormField.STORAGE,
];

export const MAIN_FORM_GRID_TEMPLATE_COLUMNS = {
	default: `${MAIN_FORM_GRID_INPUT_WIDTH} ${MAIN_FORM_GRID_SMALL_INPUT_WIDTH_LEFT} ${MAIN_FORM_GRID_SMALL_INPUT_WIDTH_MIDDLE} ${MAIN_FORM_GRID_SMALL_INPUT_WIDTH_INVENTORY}`,
	defaultSecondRow: `${MAIN_FORM_GRID_INPUT_WIDTH} ${MAIN_FORM_GRID_INPUT_WIDTH} ${MAIN_FORM_GRID_SECOND_ROW_EXTRA}`,
	defaultThirdRow: `${MAIN_FORM_GRID_SMALL_INPUT_WIDTH_LEFT} ${MAIN_FORM_GRID_SMALL_INPUT_WIDTH_MIDDLE} ${MAIN_FORM_GRID_INPUT_WIDTH} 1fr`,
	tablet: `${MAIN_FORM_GRID_INPUT_WIDTH} ${MAIN_FORM_GRID_SMALL_TABLET_INPUT_WIDTH} ${MAIN_FORM_GRID_SMALL_TABLET_INPUT_WIDTH}`,
	tabletFirstRow: `${MAIN_FORM_GRID_INPUT_WIDTH} ${MAIN_FORM_GRID_SMALL_TABLET_INPUT_WIDTH} ${MAIN_FORM_GRID_SMALL_TABLET_INPUT_WIDTH}`,
	mobile: '100%',
	equalRow: `repeat(${COLUMNS_AMOUNT}, calc(25% - (${
		COLUMN_GAP / COLUMNS_AMOUNT
	})px))`,
};

export const MAIN_FORM_GRID_TEMPLATE_AREAS = {
	default: `
        '${MainFormField.NAME} ${MainFormField.TYPE} ${MainFormField.LOCATION} ${MainFormField.STATUS}'
        '${MainFormField.INVOICE_NUMBER} ${MainFormField.SERIAL_NUMBER} ${MainFormField.SERIAL_NUMBER}  ${MainFormField.INVENTORY_NUMBER}'
        '${MainFormField.THIRD_ROW} ${MainFormField.THIRD_ROW} ${MainFormField.THIRD_ROW} ${MainFormField.THIRD_ROW}'
        '${MainFormField.FOURTH_ROW} ${MainFormField.FOURTH_ROW} ${MainFormField.FOURTH_ROW} ${MainFormField.FOURTH_ROW}'
        '${MainFormField.CHARACTERISTICS} ${MainFormField.CHARACTERISTICS} ${MainFormField.CHARACTERISTICS} ${MainFormField.CHARACTERISTICS}'
        '${MainFormField.JIRA_TASK} ${MainFormField.JIRA_TASK} ${MainFormField.JIRA_TASK} ${MainFormField.JIRA_TASK}'
        '${MainFormField.COMMENT} ${MainFormField.COMMENT} ${MainFormField.COMMENT} ${MainFormField.COMMENT}'
    `,
	defaultThirdRow: `
        '${MainFormField.PURCHASE_DATE} ${MainFormField.GUARANTEE_DATE} ${MainFormField.APPROVER} ${MainFormField.STORAGE}'
    `,
	defaultFourthRow: `
        '${MainFormField.PURCHASE_PRICE} ${MainFormField.PRICE_USD} ${MainFormField.PURCHASE_PLACE} ${MainFormField.REPAIR_PLACE}'
    `,
	tablet: `
      '${MainFormField.NAME} ${MainFormField.TYPE} ${MainFormField.LOCATION}'
      '${MainFormField.STATUS} ${MainFormField.INVOICE_NUMBER} ${MainFormField.INVOICE_NUMBER}'
      '${MainFormField.SERIAL_NUMBER} ${MainFormField.INVENTORY_NUMBER} ${MainFormField.INVENTORY_NUMBER}'
      '${MainFormField.THIRD_ROW} ${MainFormField.THIRD_ROW} ${MainFormField.THIRD_ROW}'
      '${MainFormField.FOURTH_ROW} ${MainFormField.FOURTH_ROW} ${MainFormField.FOURTH_ROW}'
      '${MainFormField.CHARACTERISTICS} ${MainFormField.CHARACTERISTICS} ${MainFormField.CHARACTERISTICS}'
      '${MainFormField.JIRA_TASK} ${MainFormField.JIRA_TASK} ${MainFormField.JIRA_TASK}'
      '${MainFormField.COMMENT} ${MainFormField.COMMENT} ${MainFormField.COMMENT}'
    `,
	tabletThirdRow: `
        '${MainFormField.PURCHASE_DATE} ${MainFormField.GUARANTEE_DATE} ${MainFormField.GUARANTEE_DATE}'
        '${MainFormField.APPROVER} ${MainFormField.STORAGE} ${MainFormField.STORAGE}'
    `,
	tabletFourthRow: `
        '${MainFormField.PURCHASE_PRICE} ${MainFormField.PRICE_USD} ${MainFormField.PRICE_USD}'
        '${MainFormField.PURCHASE_PLACE} ${MainFormField.REPAIR_PLACE} ${MainFormField.REPAIR_PLACE}'
    `,
	mobile: `
        '${MainFormField.NAME}'
        '${MainFormField.TYPE}'
        '${MainFormField.LOCATION}'
        '${MainFormField.STATUS}'
        '${MainFormField.INVOICE_NUMBER}'
        '${MainFormField.SERIAL_NUMBER}'
        '${MainFormField.INVENTORY_NUMBER}'
        '${MainFormField.THIRD_ROW}'
        '${MainFormField.FOURTH_ROW}'
        '${MainFormField.CHARACTERISTICS}'
        '${MainFormField.JIRA_TASK}'
        '${MainFormField.COMMENT}'
    `,
	mobileThirdRow: `
        '${MainFormField.PURCHASE_DATE}'
        '${MainFormField.GUARANTEE_DATE}'
        '${MainFormField.APPROVER}'
        '${MainFormField.STORAGE}'
    `,
	mobileFourthRow: `
        '${MainFormField.PURCHASE_PRICE}'
        '${MainFormField.PRICE_USD}'
        '${MainFormField.PURCHASE_PLACE}'
        '${MainFormField.REPAIR_PLACE}'
    `,
};

export const MAIN_FORM_GRID_DEFAULT_AREAS = [
	MainFormField.NAME,
	MainFormField.TYPE,
	MainFormField.LOCATION,
	MainFormField.STATUS,
	MainFormField.INVOICE_NUMBER,
	MainFormField.SERIAL_NUMBER,
	MainFormField.INVENTORY_NUMBER,
	MainFormField.THIRD_ROW,
	MainFormField.FOURTH_ROW,
	MainFormField.CHARACTERISTICS,
	MainFormField.JIRA_TASK,
	MainFormField.COMMENT,
];

export const MAIN_FORM_GRID_THIRD_ROW_AREAS = [
	MainFormField.PURCHASE_DATE,
	MainFormField.GUARANTEE_DATE,
	MainFormField.APPROVER,
	MainFormField.STORAGE,
];

export const MAIN_FORM_GRID_FOURTH_ROW_AREAS = [
	MainFormField.PURCHASE_PRICE,
	MainFormField.PRICE_USD,
	MainFormField.PURCHASE_PLACE,
	MainFormField.REPAIR_PLACE,
];

export const EQUIPMENT_USER_FORM_MAX_WIDTH = 350;

export const EQUIPMENT = EquipmentTabs.EQUIPMENT;
export const USER = EquipmentTabs.USER;

export const POPUP_TABLET_WIDTH = 756;

export const USER_TAB_HEIGHT = 578;

export const USER_TAB_TABLET_HEIGHT = 770;

export const USER_TAB_MOBILE_HEIGHT = 1218;

export const entityName = 'Equipment';

export const popupFormKeys = {
	main: 'mainForm',
	user: 'userForm',
} as const;
