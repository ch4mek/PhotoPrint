import { useTheme } from '@emotion/react';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { getCellColorByAmount } from '../../../StorageTab/utils/getCellColorByAmount';
import { getCellColorForRow } from '../../../StorageTab/utils/getCellColorForRow';
import { MerchKitsGridRow } from '../../types';

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
import { MerchStatuses } from '@/pages/Merch/constants';

export function SizeCell(props: RenderCellProps<MerchKitsGridRow>) {
	const { row } = props;
	const { expanded, isClothing, size, possibleKits, status: rowStatus } = row;
	const {
		configurableValues: { merchSizes },
	} = useConfiguration();
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(possibleKits, theme, expanded),
		[row, theme]
	);

	const getCellColorBySubIndex = (index: number) => {
		const { possibleKitAmount, status } = possibleKits[index];

		return getCellColorByAmount(theme, possibleKitAmount, status);
	};

	const isChevronShown =
		(isClothing || possibleKits.length > 1) && Boolean(possibleKits.length);

	const isDisabled = rowStatus === MerchStatuses.archived;

	return (
		<SizeCellWrapper
			disabled={isDisabled}
			data-expanded={expanded}
			color={cellColor}
		>
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
