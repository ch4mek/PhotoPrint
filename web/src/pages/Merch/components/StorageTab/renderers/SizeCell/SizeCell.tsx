import { useTheme } from '@emotion/react';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchStorageGridRow } from '../../types';
import { getCellColorByAmount } from '../../utils/getCellColorByAmount';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { useConfiguration } from '@/context/Configuration';
import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	FirstRowSizesWrapper,
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/SizeCell/styles';
import { filterSizeAndJoinWithComma } from '@/pages/Merch/components/ElementsTab/utils/filterSizesAndJoinWithComma';

export function SizeCell(props: RenderCellProps<MerchStorageGridRow>) {
	const { row } = props;
	const { expanded, isClothing, size, sizeAmounts } = row;
	const {
		configurableValues: { merchSizes },
	} = useConfiguration();
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	const getCellColorBySubIndex = (index: number) => {
		const { amount, status } = sizeAmounts[index];

		return getCellColorByAmount(theme, amount, status);
	};

	const isChevronShown =
		(isClothing || sizeAmounts.length > 1) && Boolean(sizeAmounts.length);

	return (
		<SizeCellWrapper data-expanded={expanded} color={cellColor}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							{isClothing && (
								<FirstRowSizesWrapper>
									{filterSizeAndJoinWithComma(merchSizes)}
								</FirstRowSizesWrapper>
							)}
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isChevronShown &&
					expanded &&
					size.map((item, index) => (
						<SizeSubFields
							key={`${item}.${index}`}
							color={isClothing ? getCellColorBySubIndex(index) : ''}
						>
							{isClothing && item}
						</SizeSubFields>
					))}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
