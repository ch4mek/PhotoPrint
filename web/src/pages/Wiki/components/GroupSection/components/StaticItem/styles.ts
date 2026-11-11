import styled from '@emotion/styled';

import { ItemContentWrapper } from '@/pages/Wiki/components/GroupSection/styles';

export const StaticContentWrapper = styled(ItemContentWrapper)<{
	isSelected: boolean;
}>(({ theme, isSelected }) => ({
	color: isSelected
		? theme.palette['text-accent']
		: theme.palette['text-black'],

	'&:hover': !isSelected && {
		color: theme.palette['text-gray'],
	},
}));
