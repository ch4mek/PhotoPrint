import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';

import { cellWithTooltipRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipRenderer';

import { ColorCircle, ColorCircleCellContentWrapper } from './styles';
import {
	CellWithColorCircleProps,
	CellWithColorCircleRendererOptions,
} from './types';

export function cellWithColorCircleRenderer<T extends CellWithColorCircleProps>(
	props: RenderCellProps<T, unknown>,
	options?: CellWithColorCircleRendererOptions<T>
) {
	const { fieldToDisplay, onCellClick, enableCellClick } = options ?? {};
	const { row } = props;
	const { color, showSkeleton } = row;

	const enableClick = enableCellClick ? enableCellClick(row) : true;

	const textWithTooltip = cellWithTooltipRenderer(props, {
		fieldToDisplay,
		onCellClick: enableClick ? onCellClick : undefined,
	});

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper>
			<ColorCircleCellContentWrapper>
				<ColorCircle color={color} />
				{textWithTooltip}
			</ColorCircleCellContentWrapper>
		</CellWrapper>
	);
}
