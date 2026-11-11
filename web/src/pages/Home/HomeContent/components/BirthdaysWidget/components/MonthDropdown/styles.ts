import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';

export const Container = styled('li')(({ theme }) => ({
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-white']
			: theme.palette['background-base-1'],
	borderRadius: BorderRadiuses.m,
	boxSizing: 'border-box',
	color: theme.palette['text-black'],
	paddingBlock: theme.spaces.s,
	paddingInline: theme.spaces.m,
	listStyleType: 'none',
}));

export const MonthHeader = styled('div')({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	cursor: 'pointer',
});

export const BirthdayDatesList = styled('ul')(({ theme }) => ({
	padding: 0,
	marginBottom: 0,
	marginTop: theme.spaces.s,
}));
