import { Carousel } from '@mms/mms-ui-library/components/Carousel';
import { PropsWithChildren, useMemo } from 'react';

import { useResize } from '@/hooks/useResize';

import { PAGE_PADDINGS, WIDGET_PADDINGS } from './constants';

export function WidgetCarousel({ children }: PropsWithChildren) {
	const [windowWidth] = useResize();

	const sliderWidth = useMemo(
		() => windowWidth - (PAGE_PADDINGS + WIDGET_PADDINGS) * 2,
		[windowWidth]
	);

	return (
		<Carousel {...{ sliderWidth }} slideStyles={{ maxWidth: '100%' }}>
			{children}
		</Carousel>
	);
}
