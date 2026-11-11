import styled from '@emotion/styled';
import { lightTheme } from '@mms/mms-ui-library/theme';

export const Title = styled('h6')(({ theme }) => ({
	...theme.typography.headlines.small,
	color: lightTheme.palette['text-white'],
	margin: 0,
}));
