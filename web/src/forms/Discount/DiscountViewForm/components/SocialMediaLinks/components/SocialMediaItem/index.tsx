import { SocialMediaItemWrapper } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/styles';
import { SocialMediaItemProps } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/types';
import { getSocialMediaIcon } from '@/forms/Discount/DiscountViewForm/components/SocialMediaLinks/utils/getSocialMediaIcon';

export function SocialMediaItem({ link }: SocialMediaItemProps) {
	const icon = getSocialMediaIcon(link);

	return (
		<SocialMediaItemWrapper
			href={link}
			target="_blank"
			rel="noopener noreferrer"
		>
			{icon}
		</SocialMediaItemWrapper>
	);
}
