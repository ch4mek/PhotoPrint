import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';
import { MouseEvent } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';
import { GridCellDefaultProps } from '../../types';

import {
	CellCrosslinkType,
	CellTextCutOrientation,
	CROSS_LINK_CELL_CLASS,
	STATIC_CROSS_LINK_CELL_CLASS,
	TOOLTIP_FALLBACK_PLACEMENTS,
	TOOLTIP_SHOW_DELAY,
} from '@/components/Grid/constants';
import { CellWithTooltipRendererOptions } from '@/components/Grid/renderers/cell/cellWithTooltipRenderer/types';
import { getPortalContainer } from '@/components/Grid/utils/getPortalContainer';
import { zIndexes } from '@/constants/zIndexes';

export const cellWithTooltipRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	{
		onCellClick,
		crosslinkType = CellCrosslinkType.Dynamic,
		isCopyEnabled = false,
		tooltipPortalContainerSelector,
		fieldToDisplay,
		textCutOrientation = CellTextCutOrientation.Horizontal,
		isClickable = true,
	}: CellWithTooltipRendererOptions<T> = {}
) => {
	const info = row[fieldToDisplay || (key as keyof T)] as string;
	const isLoading = row.showSkeleton;
	const crosslinkCellClass =
		crosslinkType === CellCrosslinkType.Dynamic
			? CROSS_LINK_CELL_CLASS
			: STATIC_CROSS_LINK_CELL_CLASS;

	const handleClick = (e: MouseEvent<HTMLDivElement>) => {
		if (isCopyEnabled) {
			navigator.clipboard.writeText(
				(e.target as unknown as HTMLDivElement).textContent as string
			);
		}

		if (!row.disabled && isClickable) {
			onCellClick?.(row, key, e);
		}
	};

	const isClickAllowed = !!onCellClick && !row.disabled && isClickable;

	return isLoading ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={info}
			position="bottom"
			getPortalContainer={
				tooltipPortalContainerSelector
					? getPortalContainer(tooltipPortalContainerSelector)
					: undefined
			}
			closeOnAnchorTouchEnd={isClickAllowed}
			showDelay={isClickAllowed ? TOOLTIP_SHOW_DELAY : undefined}
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			textCutOrientation={textCutOrientation}
			useIfTextCut
		>
			<CellWrapper
				onClick={handleClick}
				disabled={row.disabled}
				className={isClickAllowed ? crosslinkCellClass : undefined}
				width="100%"
			>
				{row.showCell === false ? '' : info}
			</CellWrapper>
		</Tooltip>
	);
};
