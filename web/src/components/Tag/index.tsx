import { PropsWithChildren } from 'react';

import { TagWrapper } from '@/components/Tag/styles';
import { TagProps } from '@/components/Tag/types';

export function Tag({ children, ...props }: PropsWithChildren<TagProps>) {
	return <TagWrapper {...props}>{children}</TagWrapper>;
}
