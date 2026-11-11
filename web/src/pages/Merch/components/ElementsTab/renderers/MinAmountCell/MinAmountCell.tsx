import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { FIRST_ROWS_HEIGHT } from '../../constants';
import { MerchElementsGridRow } from '../../types';
import {
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '../SizeCell/styles';

import { MinAmountTypography } from './styles';

export function MinAmountCell(props: RenderCellProps<MerchElementsGridRow>) {
	const { row } = props;
	const { expanded, storage, isClothing, odpItemSizeMinAmounts } = row;
	const notClothingMinAmounts = odpItemSizeMinAmounts
		.map(({ minAmount }) => minAmount)
		.join(', ');

	return (
		<SizeCellWrapper data-expanded={expanded}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							<Tooltip useIfTextCut text={notClothingMinAmounts}>
								<MinAmountTypography variant="m-400">
									{!isClothing && notClothingMinAmounts}
								</MinAmountTypography>
							</Tooltip>
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isClothing &&
					expanded &&
					storage.map((item) => {
						const filteredOdpItems = odpItemSizeMinAmounts.filter(
							({ warehouseLocation }) => warehouseLocation === item
						);

						return filteredOdpItems.map(({ sizeId, minAmount }) => (
							<SizeSubFields key={`${item}.${sizeId}`}>
								{minAmount}
							</SizeSubFields>
						));
					})}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
