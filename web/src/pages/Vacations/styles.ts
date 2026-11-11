import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { overrideGridStyles } from '@/components/Grid';

import { NAME_CELL_BACKGROUND } from './constants';
import { VacationTypeWrapperProps } from './types';

export const overrideVacationsGridStyles = (theme: Theme) =>
	css`
		${overrideGridStyles(false)(theme).styles};
		& .rdg-column[aria-colindex='1'] {
			background-color: ${NAME_CELL_BACKGROUND[theme.mode]};
			width: 200px;
		}

		.rdg-cell {
			border-bottom: 1px solid lightGray !important;
			font-size: ${theme.typography.body['s-400'].fontSize};
			padding: 0;
		}

		.rdg-cell[data-vacation='true'] {
			background-color: #90ee90;
		}

		.rdg-cell[role='columnheader'] {
			display: flex;
			justify-content: center;
		}
	`;

export const NameCellWrapper = styled(Typography)(({ theme }) => ({
	...theme.typography.body['m-400'],
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	width: '200px',
	display: 'flex',
	alignItems: 'center',
}));

export const VacationTypeWrapper = styled('div')<VacationTypeWrapperProps>(
	({ backgroundColor, textColor }) => ({
		width: '100%',
		height: '80%',
		backgroundColor,
		color: textColor,
		position: 'relative',
	})
);

export const VacationTypeText = styled.div`
	position: absolute;
	top: 50%;
	left: 50%;
	transform: translate(-50%, -50%);
	font-weight: bold;
	font-size: 10px;
	line-height: 1.4;
	white-space: normal;
`;

export const DateFilterWrapper = styled.div`
	width: 200px;
`;

export const VacationsHeaderWrapper = styled('div')(({ theme }) => ({
	marginBottom: theme.spaces.m,
}));
