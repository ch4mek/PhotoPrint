import styled from '@emotion/styled';

export const SkillSeniorityRow = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	gap: theme.spaces.s,
}));

export const SkillsListWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));
