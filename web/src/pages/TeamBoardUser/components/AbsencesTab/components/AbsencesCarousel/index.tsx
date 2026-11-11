import { Carousel } from '@mms/mms-ui-library/components/Carousel';

import { AbsencesCarouselProps } from './types';

export function AbsencesCarousel({
	children,
	sliderWidth,
	slidesToShow,
}: AbsencesCarouselProps) {
	return (
		<Carousel isControlVisible enableScroll {...{ sliderWidth, slidesToShow }}>
			{children}
		</Carousel>
	);
}
