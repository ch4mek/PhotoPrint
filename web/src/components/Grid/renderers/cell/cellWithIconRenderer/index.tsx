import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';
import {
	CellTextCutOrientation,
	CROSS_LINK_CELL_CLASS,
	TOOLTIP_FALLBACK_PLACEMENTS,
	TOOLTIP_SHOW_DELAY,
} from '@/components/Grid/constants';
import {
	CellTextWrapper,
	CellWithIconWrapper,
} from '@/components/Grid/renderers/cell/cellWithIconRenderer/styles';
import { getPortalContainer } from '@/components/Grid/utils/getPortalContainer';
import { zIndexes } from '@/constants/zIndexes';

import { CellWithIconRendererOptions } from './types';

export const cellWithIconRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	options?: CellWithIconRendererOptions<T>
) => {
	const {
		onIconClick,
		icon,
		tooltipPortalContainerSelector,
		fieldToDisplay,
		textCutOrientation = CellTextCutOrientation.Horizontal,
		onContentClick,
	} = options ?? {};

	const info = row[fieldToDisplay || (key as keyof T)] as string;
	const RowIcon = icon ?? PlusIcon;

	const handleRowClick = (handler?: (row: T) => void) => () => handler?.(row);
	const isClickable = !!onContentClick;

	const isClickAllowed = !row.disabled && !!handleRowClick;

	return (
		<CellWithIconWrapper disabled={!!row.disabled}>
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={info}
				position="bottom"
				getPortalContainer={
					tooltipPortalContainerSelector
						? getPortalContainer(tooltipPortalContainerSelector)
						: undefined
				}
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				textCutOrientation={textCutOrientation}
				showDelay={isClickAllowed ? TOOLTIP_SHOW_DELAY : undefined}
				closeOnAnchorTouchEnd={isClickAllowed}
				useIfTextCut
			>
				<CellTextWrapper
					variant="m-400"
					className={isClickable ? CROSS_LINK_CELL_CLASS : ''}
					onClick={handleRowClick(onContentClick)}
				>
					{info}
				</CellTextWrapper>
			</Tooltip>
			<ActionButton
				onClick={handleRowClick(onIconClick)}
				size="small"
				disabled={row.disabled}
			>
				<RowIcon />
			</ActionButton>
		</CellWithIconWrapper>
	);
};
