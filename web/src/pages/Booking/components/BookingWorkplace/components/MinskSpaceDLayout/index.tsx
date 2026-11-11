import { PropsWithChildren } from 'react';

import { MinskSpaceDLayoutWrapper } from './styled';

export function MinskSpaceDLayout({ children }: PropsWithChildren) {
	return <MinskSpaceDLayoutWrapper>{children}</MinskSpaceDLayoutWrapper>;
}
