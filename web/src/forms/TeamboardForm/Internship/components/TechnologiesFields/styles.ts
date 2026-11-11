import styled from '@emotion/styled';

export const TechnologyFieldsRow = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: '1fr 1fr auto',
	alignItems: 'center',
	gap: theme.spaces.s,
	width: '100%',
}));

export const TechnologiesListWrapper = styled('div')<{
	isLastElement: boolean;
}>(({ theme, isLastElement }) => ({
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,

	'& > *:last-child': {
		...(!isLastElement && {
			marginTop: theme.spaces.s - theme.spaces.l,
		}),
	},
}));

export const TechnologyInputsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
	width: '100%',
}));

export const AddMoreButtonWrapper = styled('div')({
	width: '100%',
});
