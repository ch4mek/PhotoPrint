import styled from '@emotion/styled';

export const TextButtonWrapper = styled('div')<{ fullWidth: boolean }>(
	({ fullWidth, theme }) => ({
		display: 'inline-block',
		width: fullWidth ? '100%' : 'auto',

		'& button span': {
			fontWeight: theme.typography.body['s-400'].fontWeight,
		},
	})
);
