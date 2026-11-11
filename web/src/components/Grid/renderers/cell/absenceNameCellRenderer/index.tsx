import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { MouseEvent } from 'react';
import { RenderCellProps } from 'react-data-grid';

import {
	CROSS_LINK_CELL_CLASS,
	TOOLTIP_SHOW_DELAY,
} from '@/components/Grid/constants';
import { ThemeMode } from '@/theme';

import { AbsenceCellWrapper, IconContainer, TextOverflow } from './styles';
import { AbsenceNameCellProps, AbsenceNameCellRendererOptions } from './types';
import { getAbsenceConfig } from './utils/getAbsenceConfig';
import { getAbsenceTypeFromValue } from './utils/getAbsenceTypeFromValue';

export const absenceNameCellRenderer = <T extends AbsenceNameCellProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	{ onCellClick, isClickable = true }: AbsenceNameCellRendererOptions<T> = {}
) => {
	const theme = useTheme();
	const mode: ThemeMode = theme.mode;
	const value = row[key as keyof T] as string;

	const absenceType = getAbsenceTypeFromValue(row.inquiryType);

	const { Icon, color, background } = getAbsenceConfig(
		theme,
		mode,
		absenceType
	);

	const handleClick = (e: MouseEvent<HTMLDivElement>) => {
		if (!row.disabled && onCellClick && isClickable) {
			onCellClick(row, key, e);
		}
	};

	const isClickAllowed = !!onCellClick && isClickable && !row.disabled;

	return (
		<AbsenceCellWrapper
			onClick={handleClick}
			className={isClickAllowed ? CROSS_LINK_CELL_CLASS : undefined}
		>
			<IconContainer {...{ background, color }}>
				<Icon />
			</IconContainer>
			<Tooltip
				useIfTextCut
				text={value}
				closeOnAnchorTouchEnd={isClickAllowed}
				showDelay={isClickAllowed ? TOOLTIP_SHOW_DELAY : undefined}
			>
				<TextOverflow variant="m-400">{value}</TextOverflow>
			</Tooltip>
		</AbsenceCellWrapper>
	);
};
