import { MerchElementsGridRow } from '../../types';

import { cellWithTooltipRenderer } from '@/components/Grid';
import { RESIZABLE_ROW_LINE_HEIGHT } from '@/constants/commonStyles';
import { getMaxVerticalLineCount } from '@/pages/Merch/utils/getMaxVerticalLineCount';
import { DefaultCellRendererProps } from '@/pages/Payload/types';

import { CellWrapper } from './styles';

export const defaultCellRenderer = ({
	props,
	options,
}: DefaultCellRendererProps<MerchElementsGridRow>) => {
	const {
		row,
		column: { key },
	} = props;

	const { withTooltip, isResizable } = options;
	const { itemsRowHeight, expanded } = row;
	const info = row[key as keyof MerchElementsGridRow] as string;
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
};
