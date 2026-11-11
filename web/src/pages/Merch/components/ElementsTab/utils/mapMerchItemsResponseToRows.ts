import { FIRST_ROWS_HEIGHT } from '../constants';
import { MerchElement, MerchElementsGridRow } from '../types';

import { getUniqueStorages } from './getUniqueStorages';

export const mapMerchItemsResponseToRows = (
	data: MerchElement[],
	handleEdit: (itemId?: string | number) => () => void
) =>
	data.map<MerchElementsGridRow>(({ odpItemSizeMinAmounts, ...rest }) => ({
		...rest,
		expanded: false,
		storage: getUniqueStorages(odpItemSizeMinAmounts),
		itemsRowHeight: FIRST_ROWS_HEIGHT,
		size: odpItemSizeMinAmounts.map(({ sizeId }) => sizeId),
		odpItemSizeMinAmounts,
		color: '',
		handleEditClick: handleEdit,
	}));
