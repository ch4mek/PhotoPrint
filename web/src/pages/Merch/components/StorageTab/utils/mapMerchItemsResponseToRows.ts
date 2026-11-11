import { FIRST_ROWS_HEIGHT } from '../../ElementsTab/constants';
import { MerchStorage, MerchStorageGridRow } from '../types';

import { PersistentConfigurationItem } from '@/types/configuration';

import { getStatusDescription } from './getStatusDescription';
import { getUniqueStorages } from './getUniqueStorage';

export const mapMerchItemsResponseToRows = (
	data: MerchStorage[],
	statuses: PersistentConfigurationItem[],
	handleEdit: (itemId?: string | number) => () => void
) =>
	data.map<MerchStorageGridRow>(
		({ sizeAmounts, status, storageAvailabilityStatuses, ...rest }) => ({
			...rest,
			expanded: false,
			storage: getUniqueStorages(sizeAmounts),
			itemsRowHeight: FIRST_ROWS_HEIGHT,
			size: sizeAmounts.map(({ sizeId }) => sizeId),
			sizeAmounts,
			color: '',
			storageAvailabilityStatuses: storageAvailabilityStatuses.map(
				({ status: statusPerStorage, storageName }) => ({
					status: getStatusDescription(statuses, statusPerStorage),
					storageName,
				})
			),
			status: getStatusDescription(statuses, status) ?? '',
			handleEditClick: handleEdit,
		})
	);
