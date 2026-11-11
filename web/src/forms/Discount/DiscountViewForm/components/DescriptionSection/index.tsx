import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useEffect, useMemo, useRef, useState } from 'react';

import {
	ONE_LINE_HEIGHT,
	sectionTypes,
} from '@/forms/Discount/DiscountViewForm/components/DescriptionSection/constants';
import {
	CitiesSection,
	DescriptionSectionWrapper,
	DescriptionWrapper,
	DetailsInfoWrapper,
	DiscountBadge,
} from '@/forms/Discount/DiscountViewForm/components/DescriptionSection/styles';
import { DescriptionSectionProps } from '@/forms/Discount/DiscountViewForm/components/DescriptionSection/types';
import { EditorSection } from '@/forms/Discount/EditorSection';
import { formatDiscountPercent } from '@/helpers/formatDiscountPercent';

export function DescriptionSection({
	discountData,
	initialData,
	initialText,
}: DescriptionSectionProps) {
	const [alignStart, setAlignStart] = useState(false);

	const citiesRef = useRef<HTMLDivElement>(null);

	const cities = useMemo(
		() =>
			discountData &&
			discountData?.discountCities &&
			discountData.discountCities.map(({ name }) => name),
		[discountData]
	);

	const formatPercentValue = useMemo(
		() =>
			formatDiscountPercent(
				discountData?.discountPercent,
				discountData?.discountPercentTo
			),
		[discountData]
	);

	useEffect(() => {
		const el = citiesRef.current;

		if (!el) {
			return;
		}

		const observer = new ResizeObserver(() => {
			setAlignStart(el.offsetHeight > ONE_LINE_HEIGHT);
		});

		observer.observe(el);

		return () => observer.disconnect();
	}, []);

	return (
		<DescriptionSectionWrapper>
			<DetailsInfoWrapper alignStart={alignStart}>
				{formatPercentValue && (
					<DiscountBadge>{formatPercentValue}</DiscountBadge>
				)}
				<CitiesSection ref={citiesRef}>{cities?.join(', ')}</CitiesSection>
			</DetailsInfoWrapper>
			<DescriptionWrapper>
				<Typography variant="small">{sectionTypes.description}</Typography>
				{initialText && <EditorSection isReadMode initialData={initialData} />}
			</DescriptionWrapper>
		</DescriptionSectionWrapper>
	);
}
