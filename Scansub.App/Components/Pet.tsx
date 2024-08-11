import { Text, View } from "react-native"

type PetProps = {
    petName: { 
        firstName:string,
        lastName:string,
    },
    type: string
}

export const Pet = (props:PetProps) => {

    const { lastName, firstName} = props.petName;


    return (<View>
        <Text>You hav a {props.type}</Text>
        <Text>that is called { firstName} {lastName}</Text>
    </View>)
}