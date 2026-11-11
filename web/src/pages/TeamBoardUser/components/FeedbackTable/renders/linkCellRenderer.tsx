import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid/renderers/types';

import { CustomLink, LinkCellWrapper } from './styles';

export const linkCellRenderer = <T extends GridCellDefaultProps>(
	{ row }: RenderCellProps<T, unknown>,
	onLinkClick: (row: T) => void,
	hasLink: (row: T) => boolean
) => {
	const handleLinkClick = () => {
		onLinkClick(row);
	};

	return (
		<LinkCellWrapper>
			{hasLink(row) && <CustomLink onClick={handleLinkClick}>Link</CustomLink>}
		</LinkCellWrapper>
	);
};
