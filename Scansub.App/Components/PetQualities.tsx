import { View, Text } from "react-native"

export type Qualities = {
    qualOne: string,
    qualTwo: string,
    qualThree:string,
    age:number
}

type PetQualProps = {
    qualities: Qualities[]
}

export const PetQualities = (props:PetQualProps) => {
    return (
        <View>
            {props.qualities.map((e,index) => {
                if (index === 0) {
                    return <Text>Your pet is {e.qualOne}, {e.qualTwo}, {e.qualThree} and {e.age} years old</Text>
                }
                return <Text>also {e.qualOne}, {e.qualTwo}, {e.qualThree} and still {e.age} years old</Text>
            })}
        </View>
    )
}