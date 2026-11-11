import { SocialMediaItem } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/components/SocialMediaItem';
import { SocialMediaItemsWrapper } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/styles';
import { SocialMediaLinksProps } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/types';

export function SocialMediaLinks({ links }: SocialMediaLinksProps) {
	return (
		<SocialMediaItemsWrapper>
			{links.map((item, index) => (
				<SocialMediaItem key={index} link={item} />
			))}
		</SocialMediaItemsWrapper>
	);
}
