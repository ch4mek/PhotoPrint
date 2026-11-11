import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { MerchTransactionsGridRow } from '../../types';

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

export function IdRenderCell(props: RenderCellProps<MerchTransactionsGridRow>) {
	const { row } = props;
	const { expanded, orderTransactionId, transactions } = row;

	return (
		<SizeCellWrapper data-expanded={expanded}>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							<Tooltip useIfTextCut text={orderTransactionId}>
								<MinAmountTypography variant="m-400">
									{orderTransactionId}
								</MinAmountTypography>
							</Tooltip>
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{expanded &&
					transactions.map(({ id }) => (
						<SizeSubFields key={`${id}`}>{id}</SizeSubFields>
					))}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
