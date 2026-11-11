import { FIRST_ROWS_HEIGHT } from '../../ElementsTab/constants';
import { getStatusDescription } from '../../StorageTab/utils/getStatusDescription';
import { MerchKits, MerchKitsGridRow } from '../types';

import { PersistentConfigurationItem } from '@/types/configuration';

export const mapMerchItemsResponseToRows = (
	data: MerchKits[],
	statuses: PersistentConfigurationItem[],
	handleEdit: (itemId?: string | number) => () => void,
	handleTransactionAdd: (kitId?: string | number) => () => void
) =>
	data.map<MerchKitsGridRow>(
		({
			allStorages,
			totalPossibleKits,
			status,
			containsClothing,
			compounds,
			possibleKits,
			storageAvailabilityStatuses,
			...rest
		}) => ({
			...rest,
			expanded: false,
			storage: allStorages,
			itemRowHeight: FIRST_ROWS_HEIGHT,
			totalPossibleKits,
			storageAvailabilityStatuses: storageAvailabilityStatuses.map(
				({ status: statusPerStorage, storageName }) => ({
					status: getStatusDescription(statuses, statusPerStorage),
					storageName,
				})
			),
			size: possibleKits.map(({ sizeId }) => sizeId),
			compound: compounds
				.map(({ amountInKit, name }) => `${name} (${amountInKit})`)
				.join(', '),
			isClothing: containsClothing,
			possibleKits,
			color: '',
			handleEdit,
			handleTransactionAdd,
			status: statuses.find(({ name }) => name === status)?.description ?? '',
		})
	);
