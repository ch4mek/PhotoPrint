import styled from '@emotion/styled';

export const ProgrammerDayGameWrapper = styled('section')({
	display: 'flex',
	width: '100%',
});

export const ProgrammerDayGameFrame = styled('iframe')(({ theme }) => ({
	display: 'block',
	width: '100%',
	border: 0,
	borderRadius: theme.spaces.s,
	backgroundColor: theme.palette['background-base-1'],
}));
