import styled from '@emotion/styled';

export const ImageUploadWrapper = styled('div')({});

export const UploadWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',

	gap: theme.spaces.s,
}));

export const HiddenInput = styled('input')({});
