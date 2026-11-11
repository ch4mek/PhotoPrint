import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useMemo } from 'react';

import { DevelopmentPlanItem } from '../DevelopmentPlanItem';

import { HOME_MOBILE_BREAKPOINT } from '@/pages/Home/HomeContent/constants';

import {
	DevelopmentPlansListWrapper,
	DevelopmentPlansWidgetCarouselWrapper,
} from './styles';
import { DevelopmentPlanListProps } from './types';

export function DevelopmentPlansList({
	data,
	handleSendToReview,
}: DevelopmentPlanListProps) {
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);

	const itemsRender = useMemo(
		() =>
			data.map((item) => (
				<DevelopmentPlanItem
					key={item.id}
					{...{ item, handleSendToReview, isDesktopView }}
				/>
			)),
		[data, isDesktopView, handleSendToReview]
	);

	return isDesktopView ? (
		<DevelopmentPlansListWrapper>{itemsRender}</DevelopmentPlansListWrapper>
	) : (
		<DevelopmentPlansWidgetCarouselWrapper>
			{itemsRender}
		</DevelopmentPlansWidgetCarouselWrapper>
	);
}
