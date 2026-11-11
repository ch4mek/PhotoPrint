import { css } from '@emotion/react';
import styled from '@emotion/styled';
import {
	Scrollbar,
	getScrollbarStyles,
} from '@mms/mms-ui-library/components/Scrollbar';

import { ControlButtons } from '../../components';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

const optionSectionSpaces = {
	s: 6,
	m: 7,
	l: 10,
};

const CHECKBOX_SIZE = 20;

export const SelectionModalWrapper = styled('div')<{ maxHeight?: number }>(
	({ maxHeight }) => css`
		max-height: ${maxHeight ? `${maxHeight}px` : 'auto'};
		display: flex;
		flex-direction: column;
	`
);

export const SelectsSections = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	padding: `${optionSectionSpaces.l}px ${theme.spaces.s}px`,
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
}));

export const OptionSection = styled('div')(
	({ theme }) => css`
		padding: ${theme.spaces.s}px;
		border-radius: ${BorderRadiuses.m}px;
		cursor: pointer;

		& > * {
			pointer-events: none !important;
		}

		&:hover {
			background-color: ${theme.palette['hover-base-2']};

			& span {
				background-color: ${theme.palette['background-white']};
			}
		}
	`
);

export const OptionsSection = styled(Scrollbar)(
	({ theme }) => css`
		padding: ${theme.spaces.s}px;
		display: flex;
		flex-direction: column;
		padding-right: ${optionSectionSpaces.s}px;
		${getScrollbarStyles(theme, scrollbarWidths.s, 'vertical')};

		span {
			margin: 0;
			width: ${CHECKBOX_SIZE}px;
			height: ${CHECKBOX_SIZE}px;
		}
	`
);

export const ControlButtonsWrapper = styled(ControlButtons)(
	({ theme }) => css`
		display: flex;
		padding: ${theme.spaces.s}px ${theme.spaces.m}px ${theme.spaces.m}px;
	`
);
