import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { ChevronIcon } from '@mms/mms-ui-library/icons/base/ChevronIcon';
import { RenderCellProps } from 'react-data-grid';

import { InterviewGridRow } from '../../types';

import { ChevronButton } from '@/components/ChevronButton';
import {
	CROSS_LINK_CELL_CLASS,
	TOOLTIP_SHOW_DELAY,
} from '@/components/Grid/constants';
import { CellWithIconRendererOptions } from '@/components/Grid/renderers/cell/cellWithIconRenderer/types';

import { CellWrapper, UserText } from './styled';

export function UserCellWithExpand(
	{ row, onRowChange }: RenderCellProps<InterviewGridRow>,
	options: CellWithIconRendererOptions<InterviewGridRow>
) {
	const theme = useTheme();

	const { onContentClick } = options;
	const { user, hasStages, expanded, isMainInterview, canViewPersonalInfo } =
		row;

	const handleExpand = () => {
		if (onRowChange) {
			onRowChange({ ...row, expanded: !row.expanded });
		}
	};

	const handleContentClick = () => {
		if (canViewPersonalInfo && isMainInterview) {
			onContentClick?.(row);
		}
	};

	const isClickAllowed =
		!!onContentClick && canViewPersonalInfo && isMainInterview;
	const crosslinkCellClass = CROSS_LINK_CELL_CLASS;

	return (
		<CellWrapper>
			<Tooltip
				text={user}
				useIfTextCut
				closeOnAnchorTouchEnd={isClickAllowed}
				showDelay={isClickAllowed ? TOOLTIP_SHOW_DELAY : undefined}
			>
				<UserText
					onClick={handleContentClick}
					className={isClickAllowed ? crosslinkCellClass : undefined}
				>
					{user}
				</UserText>
			</Tooltip>
			{isMainInterview && hasStages && (
				<ChevronButton
					onClick={handleExpand}
					chevronColor={theme.palette['graphics-primary-2']}
					isActive={expanded}
					size="small"
				>
					<ChevronIcon />
				</ChevronButton>
			)}
		</CellWrapper>
	);
}
