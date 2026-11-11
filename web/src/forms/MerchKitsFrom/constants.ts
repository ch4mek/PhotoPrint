import {
	getActivatingPopUpTitle,
	getArchivingPopUpTitle,
} from '@/constants/activityStatus';
import {
	getSuccessfullyActivatedMessage,
	getSuccessfullyArchivedMessage,
} from '@/constants/notifications';
import { MerchStatuses } from '@/pages/Merch/constants';
import { MerchStorageType } from '@/types/configuration';

import { MerchKitAction, MerchKitActionConfig } from './types';

export const MERCH_KITS_FIELD_TITLE = 'Kit';

export const FORM_NAME = 'Merch kit form';

const KIT_ARCHIVE_CONFIRM_MESSAGE =
	'Are you sure you want to archive this kit?';
const KIT_ACTIVATE_CONFIRM_MESSAGE =
	'Are you sure you want to activate this kit?';

export const merchKitActionConfig: Record<
	MerchKitAction,
	MerchKitActionConfig
> = {
	[MerchKitAction.Archive]: {
		buttonText: 'Archive',
		popUpTitle: getArchivingPopUpTitle(MERCH_KITS_FIELD_TITLE),
		confirmMessage: KIT_ARCHIVE_CONFIRM_MESSAGE,
		successMessage: getSuccessfullyArchivedMessage(MERCH_KITS_FIELD_TITLE),
		targetStatus: MerchStatuses.archived,
	},
	[MerchKitAction.Activate]: {
		buttonText: 'Activate',
		popUpTitle: getActivatingPopUpTitle(MERCH_KITS_FIELD_TITLE),
		confirmMessage: KIT_ACTIVATE_CONFIRM_MESSAGE,
		successMessage: getSuccessfullyActivatedMessage(MERCH_KITS_FIELD_TITLE),
		targetStatus: MerchStatuses.available,
	},
};

export const getMerchKitActionByStatus = (
	currentStatus: string | undefined
): MerchKitAction | null => {
	if (currentStatus === MerchStatuses.available) {
		return MerchKitAction.Archive;
	}

	if (currentStatus === MerchStatuses.archived) {
		return MerchKitAction.Activate;
	}

	return null;
};

export const getInitialFormValues = (merchStorages: MerchStorageType[]) => ({
	name: '',
	description: '',
	minAmounts: merchStorages.map(({ id }) => ({
		minimumAmount: 0,
		storageId: +id,
	})),
	photoFileId: null,
	imageChangeMarker: null,
	compounds: [{ amountInKit: 1, itemId: undefined }],
});

export const merchKitsFieldData = {
	name: {
		name: 'name',
		label: 'Kit name',
	},
	description: {
		name: 'description',
		label: 'Description',
	},
	compound: {
		label: 'Compound',
	},
	imageChangeMarker: {
		name: 'imageChangeMarker',
	},
} as const;
