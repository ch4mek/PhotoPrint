import { Typography } from '@mms/mms-ui-library/components/Typography';

import { LinkIconWrapper, LinkItemWrapper } from './styles';
import { LinkItemProps } from './types';

export function LinkItem({ icon, title, link }: LinkItemProps) {
	return (
		<LinkItemWrapper href={link} target="_blank" rel="noopener noreferrer">
			<LinkIconWrapper>{icon}</LinkIconWrapper>
			<Typography variant="m-400">{title}</Typography>
		</LinkItemWrapper>
	);
}
