import { PropsWithChildren } from 'react';

import { MinskSpaceFLayoutWrapper } from './styled';

export function MinskSpaceFLayout({ children }: PropsWithChildren) {
	return <MinskSpaceFLayoutWrapper>{children}</MinskSpaceFLayoutWrapper>;
}
