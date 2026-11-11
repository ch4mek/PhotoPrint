import styled from '@emotion/styled';

export const AbsencesWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	gap: theme.spaces.m,
	overflow: 'hidden',
}));

export const UserAbsenceItemWrapper = styled('div')<{
	maxWidth?: string | number;
}>(({ maxWidth }) => ({
	width: '100%',
	maxWidth,
	minWidth: 0,
}));
