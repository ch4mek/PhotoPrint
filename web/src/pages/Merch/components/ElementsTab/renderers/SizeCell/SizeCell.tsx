import { RenderCellProps } from 'react-data-grid';

import { FIRST_ROWS_HEIGHT } from '../../constants';
import { MerchElementsGridRow } from '../../types';
import { filterSizeAndJoinWithComma } from '../../utils/filterSizesAndJoinWithComma';

import { useConfiguration } from '@/context/Configuration';

import {
	FirstRowSizesWrapper,
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from './styles';

export const sizeCell = (props: RenderCellProps<MerchElementsGridRow>) => {
	const { row } = props;
	const { expanded, isClothing, storage, odpItemSizeMinAmounts } = row;
	const {
		configurableValues: { merchSizes },
	} = useConfiguration();

	return (
		<SizeCellWrapper data-expanded={expanded}>
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
				{isClothing &&
					expanded &&
					storage.map((item) => {
						const filteredOdpItems = odpItemSizeMinAmounts.filter(
							({ warehouseLocation }) => warehouseLocation === item
						);

						return filteredOdpItems.map(({ sizeId }) => (
							<SizeSubFields key={`${item}.${sizeId}`}>{sizeId}</SizeSubFields>
						));
					})}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
};
