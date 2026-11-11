import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { FIRST_ROWS_HEIGHT } from '../../../ElementsTab/constants';
import {
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '../../../ElementsTab/renderers/SizeCell/styles';
import { MerchTransactionsGridRow } from '../../types';
import { MinAmountTypography } from '../IdRenderCell/styles';

export function AmountCell(props: RenderCellProps<MerchTransactionsGridRow>) {
	const { row } = props;
	const { expanded, isClothing, transactions, sumAmount } = row;

	return (
		<SizeCellWrapper data-expanded={expanded}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							<Tooltip useIfTextCut text={sumAmount}>
								<MinAmountTypography variant="m-400">
									{sumAmount}
								</MinAmountTypography>
							</Tooltip>
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isClothing &&
					expanded &&
					transactions.map(({ id, amount }) => (
						<SizeSubFields key={`${id}`}>{amount}</SizeSubFields>
					))}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
