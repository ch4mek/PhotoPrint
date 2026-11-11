import { useTheme } from '@emotion/react';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { getCellColorForRow } from '../../../StorageTab/utils/getCellColorForRow';
import { MerchKitsGridRow } from '../../types';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	StorageCellWrapper,
	StorageFieldsWrapper,
	StorageGeneralFields,
	StorageGeneralFieldWrapper,
	StorageCellContentWrapper,
	StorageSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/ItemStorageCell/styles';
import { MerchStatuses } from '@/pages/Merch/constants';

export function ItemStorageCell(props: RenderCellProps<MerchKitsGridRow>) {
	const { row } = props;
	const { expanded, storage, possibleKits, isClothing, status } = row;
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(possibleKits, theme, expanded),
		[row, theme]
	);

	const isDisabled = status === MerchStatuses.archived;

	return (
		<StorageCellWrapper
			disabled={isDisabled}
			data-expanded={expanded}
			color={cellColor}
		>
			<StorageFieldsWrapper>
				<StorageGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<StorageGeneralFieldWrapper isFirstRow>
						<StorageCellContentWrapper>
							{storage.map((item) => item).join(', ')}
						</StorageCellContentWrapper>
					</StorageGeneralFieldWrapper>
				</StorageGeneralFields>
				{expanded &&
					storage.map((item) => (
						<StorageSubFields isClothing={isClothing} key={item}>
							{item}
						</StorageSubFields>
					))}
			</StorageFieldsWrapper>
		</StorageCellWrapper>
	);
}
