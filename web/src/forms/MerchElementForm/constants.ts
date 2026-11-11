import { MerchSizeType, MerchStorageType } from '@/types/configuration';

import {
	ConfirmationModalTextValue,
	MerchElementsFormValues,
	ValueByMerchElementStatus,
} from './types';
import { getInitialMerchSizeMinAmounts } from './utils/getInitialMerchSizeMinAmounts';

export const MERCH_ELEMENTS_FIELD_TITLE = 'Merch item';
export const FORM_NAME = 'Merch item form';

export const BUTTON_ARCHIVE_TEXT = 'Archive';
export const BUTTON_ACTIVATE_TEXT = 'Activate';

export const confirmationModalTextByStatus = {
	active: {
		title: 'Merch item activating',
		confirmationMessage: 'Are you sure you want to activate this item?',
	},
	archived: {
		title: 'Merch item archiving',
		confirmationMessage: 'Are you sure you want to archive this item?',
	},
	archivedWithKit: {
		title: 'Merch item archiving',
		contentText: `The Kit related to the selected item will also receive status 'Archived'`,
		confirmationMessage: 'Are you sure you want to archive this item and kit?',
	},
} satisfies ValueByMerchElementStatus<ConfirmationModalTextValue>;

export const successMessage = {
	active: 'Merch item is successfully activated',
	archived: 'Merch item is successfully archived',
	archivedWithKit: 'Merch item and related kits are successfully archived',
} as const;

export const TOOLTIP_TEXT_CONTENT =
	"Set the minimum amount for the storage. If you set it to 0, you won't get alert when it's running out";

export const getInitialFormValues = (
	merchStorages: MerchStorageType[],
	merchSizes: MerchSizeType[]
): MerchElementsFormValues => ({
	name: '',
	description: '',
	isClothing: false,
	sizeMinAmounts: getInitialMerchSizeMinAmounts(merchStorages, merchSizes),
	photoFileId: null,
	imageChangeMarker: null,
	supplier: '',
});

export const merchElementsFieldData = {
	name: {
		name: 'name',
		label: 'Name',
	},
	supplier: {
		name: 'supplier',
		label: 'Supplier',
	},
	description: {
		name: 'description',
		label: 'Description',
	},
	isClothing: {
		name: 'isClothing',
		label: 'Type “Clothing”',
	},
	imageChangeMarker: {
		name: 'imageChangeMarker',
	},
} as const;

export const SIZE_INPUT_HEIGHT = 40;

export const MIN_AMOUNT_TEXT = 'Minimum amount';
export const TEXTAREA_MIN_ROWS = 5;
