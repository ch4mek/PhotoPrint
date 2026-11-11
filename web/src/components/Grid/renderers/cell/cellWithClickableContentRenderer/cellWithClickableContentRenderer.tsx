import { Skeleton } from '@mui/material';
import { MouseEventHandler } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';

import { cellWithTooltipRenderer } from '@/components/Grid';

import { Text } from './styles';
import { CellWithSerialNumberProps } from './types';

export function cellWithClickableContentRenderer<
	T extends CellWithSerialNumberProps
>(props: RenderCellProps<T, unknown>) {
	const { row } = props;
	const { showSkeleton, handleEditClick, equipmentId } = row;

	const textWithTooltip = cellWithTooltipRenderer(props);

	const handleCellClickCapture: MouseEventHandler = (mouseEvent) => {
		mouseEvent.stopPropagation();
		handleEditClick?.(equipmentId)();
	};

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper onClickCapture={handleCellClickCapture}>
			<Text>{textWithTooltip}</Text>
		</CellWrapper>
	);
}
