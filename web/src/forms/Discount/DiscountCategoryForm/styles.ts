import styled from '@emotion/styled';

export const TagsFieldWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
}));

export const TagFieldWrapper = styled('div')({
	flex: 1,
});
