import styled from '@emotion/styled';

export const DynamicFieldsGroupWrapper = styled('div')<{ gridArea?: string }>(
	({ theme, gridArea }) => ({
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'flex-start',
		gap: theme.spaces.s,

		...{ gridArea },
	})
);

export const DynamicFieldWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const FieldArrayWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,

	width: '100%',
}));
