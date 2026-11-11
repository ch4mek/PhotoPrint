import { ReactNode } from 'react';

import { PopoverContentWrapper } from '@/components/FilterPopoverContent/styles';

export function FilterPopoverContent({ children }: { children: ReactNode[] }) {
	return <PopoverContentWrapper>{children}</PopoverContentWrapper>;
}
