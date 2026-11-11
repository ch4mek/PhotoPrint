import { Hint } from '@mms/mms-ui-library/components/Hint';

import {
	CellWithColorCircleProps,
	cellWithTooltipRenderer,
	logicCellRenderer,
} from '@/components/Grid';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { CellWithTooltipAlternativeRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipAlternativeRenderer';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import { ColorPicker } from '@/pages/Payload/components/ColorPicker';
import {
	DEPARTMENT_CELL_CHILD_WIDTH,
	TOTAL_ROW_ID,
} from '@/pages/Payload/contants';
import { CellWrapper, IdCellWrapper } from '@/pages/Payload/styles';
import {
	DefaultCellRendererProps,
	ResizableCellRendererProps,
} from '@/pages/Payload/types';
import { getMaxVerticalLineCount } from '@/pages/Payload/utils/getMaxVerticalLineCount';

export const defaultCellRenderer = <
	T extends CellWithColorCircleProps & ResizableCellRendererProps
>({
	props,
	options,
}: DefaultCellRendererProps<T>) => {
	const {
		row,
		column: { key },
	} = props;
	const {
		isResizable,
		isLogicCell,
		withCircle,
		withTooltip,
		isDepartmentCell,
	} = options!;
	const { tooltipContent, setTooltipContent, isTextCut } = useTooltipContent();
	const info = row[key as keyof T] as string;
	const logic = logicCellRenderer(props);
	const tooltip = isResizable
		? cellWithTooltipRenderer(props, {
				textCutOrientation: 'vertical',
		  })
		: CellWithTooltipAlternativeRenderer(props, setTooltipContent, false, {
				cellHeight: 'auto',
				cellMaxWidth: isDepartmentCell ? DEPARTMENT_CELL_CHILD_WIDTH : 'auto',
		  });

	if (row.id === TOTAL_ROW_ID) {
		return null;
	}

	if (isLogicCell) {
		const shouldPaintCell = row[key as keyof T] as boolean;

		return (
			<CellWrapper childHeight="auto" {...{ shouldPaintCell }}>
				{logic}
			</CellWrapper>
		);
	}

	if (withCircle) {
		const userId = Number(row.id);

		return (
			<CellWrapper childHeight="auto">
				<IdCellWrapper disabled={row.disabled || false}>
					<ColorPicker
						disabled={row.disabled || false}
						initialColor={row.color}
						userId={userId}
					/>
					{info}
				</IdCellWrapper>
			</CellWrapper>
		);
	}

	if (withTooltip) {
		const maxVerticalLineCount = getMaxVerticalLineCount(
			row.projectItemsRowHeight || 0
		);

		return (
			<CellWrapper
				childHeight="auto"
				childWidth="auto"
				maxVerticalLineCount={isResizable ? maxVerticalLineCount : null}
			>
				{tooltip}
				{!isResizable && tooltipContent?.text && isTextCut && (
					<Hint
						open
						anchorElement={tooltipContent.anchorElement}
						position="bottom"
						fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
						text={tooltipContent.text}
					/>
				)}
			</CellWrapper>
		);
	}

	return <CellWrapper>{info}</CellWrapper>;
};
