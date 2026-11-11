import styled from '@emotion/styled';

export const LinkWrapper = styled('a')(({ theme }) => ({
	color: theme.palette['text-black'],
	direction: 'rtl',
}));

export const RTLWrapper = styled('div')({
	direction: 'rtl',
});
