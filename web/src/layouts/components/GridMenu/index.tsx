import { GridContainer, GridMenuWrapper, MenuHeader } from './styles';
import { GridMenuProps } from './types';

export function GridMenu({ children, title }: GridMenuProps) {
	return (
		<GridMenuWrapper>
			{title && <MenuHeader variant={undefined}>{title}</MenuHeader>}
			<GridContainer>{children}</GridContainer>
		</GridMenuWrapper>
	);
}
