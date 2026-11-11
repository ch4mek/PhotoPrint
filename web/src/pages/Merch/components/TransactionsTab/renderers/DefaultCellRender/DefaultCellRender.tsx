import { MerchTransactionsGridRow } from '../../types';

import { cellWithTooltipRenderer } from '@/components/Grid';
import { RESIZABLE_ROW_LINE_HEIGHT } from '@/constants/commonStyles';
import { getMaxVerticalLineCount } from '@/pages/Merch/utils/getMaxVerticalLineCount';
import { DefaultCellRendererProps } from '@/pages/Payload/types';

import { CellWrapper } from './styles';

export function DefaultCellRenderer({
	props,
	options,
}: DefaultCellRendererProps<MerchTransactionsGridRow>) {
	const {
		row,
		column: { key },
	} = props;

	const { withTooltip, isResizable } = options;
	const { itemsRowHeight, expanded } = row;
	const info = row[key as keyof MerchTransactionsGridRow] as string;
	const tooltip = cellWithTooltipRenderer(props, {
		textCutOrientation: 'vertical',
	});

	if (withTooltip) {
		const maxVerticalLineCount = getMaxVerticalLineCount(itemsRowHeight || 0);

		return (
			<CellWrapper
				isExpanded={expanded}
				maxVerticalLineCount={isResizable ? maxVerticalLineCount : null}
				childHeight={`${maxVerticalLineCount * RESIZABLE_ROW_LINE_HEIGHT}px`}
				childWidth="auto"
			>
				{tooltip}
			</CellWrapper>
		);
	}

	return <CellWrapper isExpanded={expanded}>{info}</CellWrapper>;
}
