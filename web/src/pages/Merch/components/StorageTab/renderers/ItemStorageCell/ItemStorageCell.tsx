import { useTheme } from '@emotion/react';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	StorageCellWrapper,
	StorageFieldsWrapper,
	StorageGeneralFields,
	StorageGeneralFieldWrapper,
	StorageCellContentWrapper,
	StorageSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/ItemStorageCell/styles';

export function ItemStorageCell(props: RenderCellProps<MerchStorageGridRow>) {
	const { row } = props;
	const { expanded, storage, sizeAmounts, isClothing } = row;
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	return (
		<StorageCellWrapper data-expanded={expanded} color={cellColor}>
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
						<StorageSubFields key={item} isClothing={isClothing}>
							{item}
						</StorageSubFields>
					))}
			</StorageFieldsWrapper>
		</StorageCellWrapper>
	);
}
