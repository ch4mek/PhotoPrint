import { RenderCellProps } from 'react-data-grid';

import { FIRST_ROWS_HEIGHT } from '../../../ElementsTab/constants';
import {
	FirstRowSizesWrapper,
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '../../../ElementsTab/renderers/SizeCell/styles';
import { MerchTransactionsGridRow } from '../../types';
import { joinTransactionsSizesWithComma } from '../../utils/joinTransactionsSizesWithComma';

export function SizeCell(props: RenderCellProps<MerchTransactionsGridRow>) {
	const { row } = props;
	const { expanded, isClothing, transactions } = row;

	return (
		<SizeCellWrapper data-expanded={expanded}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							{isClothing && (
								<FirstRowSizesWrapper>
									{joinTransactionsSizesWithComma(transactions)}
								</FirstRowSizesWrapper>
							)}
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isClothing &&
					expanded &&
					transactions.map(({ size }, index) => (
						<SizeSubFields key={`${size}.${index}`}>{size}</SizeSubFields>
					))}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
