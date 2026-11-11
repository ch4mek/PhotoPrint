import { LinkWrapper } from './styles';
import { InfoDataLinkProps } from './types';

export const InfoDataLink = ({ href, value }: InfoDataLinkProps) => (
	<LinkWrapper href={href} target="_blank" rel="noopener noreferrer">
		{value}
	</LinkWrapper>
);
