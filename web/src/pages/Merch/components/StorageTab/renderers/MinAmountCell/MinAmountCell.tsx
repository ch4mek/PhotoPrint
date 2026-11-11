import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchStorageGridRow } from '../../types';
import { getCellColorByAmount } from '../../utils/getCellColorByAmount';
import { getCellColorForRow } from '../../utils/getCellColorForRow';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/SizeCell/styles';

import { MinAmountTypography } from './styles';

export function MinAmountCell(props: RenderCellProps<MerchStorageGridRow>) {
	const { row } = props;
	const { expanded, storage, isClothing, sizeAmounts, totalAmount } = row;
	const theme = useTheme();

	const isChevronShown =
		(isClothing || sizeAmounts.length > 1) && Boolean(sizeAmounts.length);

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	return (
		<SizeCellWrapper data-expanded={expanded} color={cellColor}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							<Tooltip useIfTextCut text={totalAmount}>
								<MinAmountTypography variant="m-400">
									{totalAmount}
								</MinAmountTypography>
							</Tooltip>
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isChevronShown &&
					expanded &&
					storage.map((item) => {
						const filteredOdpItems = sizeAmounts.filter(
							({ storageName }) => storageName === item
						);

						return filteredOdpItems.map(({ sizeId, amount, status }) => (
							<SizeSubFields
								key={`${item}.${sizeId}`}
								color={getCellColorByAmount(theme, amount, status)}
							>
								{amount}
							</SizeSubFields>
						));
					})}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
